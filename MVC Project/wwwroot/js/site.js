var bellBtn = document.getElementById('bellBtn');

if (bellBtn) {

    loadUnreadCount();

    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    connection.on("ReceiveNotification", function (message) {
        loadUnreadCount();
        showToast(message);
    });

    connection.start().catch(function (err) {
        console.error("SignalR error: " + err.toString());
    });

    function loadUnreadCount() {
        fetch('/Notifications/GetUnreadCount')
            .then(function (r) { return r.json(); })
            .then(function (count) {
                var badge = document.getElementById('bellBadge');
                if (count > 0) {
                    badge.style.display = 'flex';
                    badge.innerText = count;
                } else {
                    badge.style.display = 'none';
                }
            });
    }

    function toggleNotifications() {
        var dropdown = document.getElementById('notifDropdown');
        dropdown.classList.toggle('show');
        if (dropdown.classList.contains('show')) {
            loadNotifications();
        }
    }

    function loadNotifications() {
        fetch('/Notifications/GetAll')
            .then(function (r) { return r.json(); })
            .then(function (data) {
                var list = document.getElementById('notifList');

                if (data.length === 0) {
                    list.innerHTML = '<div class="notif-empty">No notifications yet.</div>';
                    return;
                }

                var html = '';
                data.forEach(function (n) {
                    var cssClass = n.isRead ? 'read' : 'unread';
                    var dot = n.isRead ? '' : '<span style="color:#003580;font-weight:bold;">● </span>';
                    var time = new Date(n.createdOn).toLocaleString();
                    html += '<div class="notif-item ' + cssClass + '" id="notif-' + n.id + '">'
                        + '<div class="notif-body">'
                        + dot + n.message
                        + '<div class="notif-time">' + time + '</div>'
                        + '</div>'
                        + '<button onclick="deleteNotification(' + n.id + ')" class="notif-delete-btn">✕</button>'
                        + '</div>';
                });

                list.innerHTML = html;
            });
    }

    function deleteNotification(id) {
        fetch('/Notifications/Delete?id=' + id, { method: 'POST' })
            .then(function () {
                var item = document.getElementById('notif-' + id);
                if (item) item.remove();

                var list = document.getElementById('notifList');
                if (list.children.length === 0) {
                    list.innerHTML = '<div class="notif-empty">No notifications yet.</div>';
                }

                loadUnreadCount();
            });
    }

    function markAllRead() {
        fetch('/Notifications/MarkAllRead', { method: 'POST' })
            .then(function () {
                loadUnreadCount();
                loadNotifications();
            });
    }

    function showToast(message) {
        // Remove any existing toast first
        var existing = document.getElementById('liveToast');
        if (existing) existing.remove();

        var toast = document.createElement('div');
        toast.id = 'liveToast';
        toast.className = 'notif-toast';
        toast.innerHTML = '<div style="display:flex;justify-content:space-between;align-items:flex-start;gap:10px;">'
            + '<div><strong>🔔 New Complaint</strong><div style="margin-top:6px;font-size:12px;">' + message + '</div></div>'
            + '<button onclick="document.getElementById(\'liveToast\').remove()" style="background:none;border:none;color:white;font-size:16px;cursor:pointer;line-height:1;padding:0;">✕</button>'
            + '</div>';
        document.body.appendChild(toast);

        setTimeout(function () {
            var t = document.getElementById('liveToast');
            if (t) t.remove();
        }, 8000);
    }

    document.addEventListener('click', function (e) {
        var dropdown = document.getElementById('notifDropdown');
        var bell = document.getElementById('bellBtn');
        if (!bell.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

}