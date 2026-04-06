// Notification Bell — runs only if bell exists in DOM (Admin only)
var bellBtn = document.getElementById('bellBtn');

if (bellBtn) {

    // Load unread count on page load
    loadUnreadCount();

    // SignalR — listen for real-time new complaints
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .build();

    connection.on("ReceiveNotification", function (message) {
        loadUnreadCount();
        showToast(message);  // show popup when new complaint arrives
    });

    connection.start().catch(function (err) {
        console.error("SignalR error: " + err.toString());
    });

    // Load unread count and show/hide badge
    function loadUnreadCount() {
        fetch('/Notifications/GetUnreadCount')
            .then(r => r.json())
            .then(count => {
                var badge = document.getElementById('bellBadge');
                if (count > 0) {
                    badge.style.display = 'flex';
                    badge.innerText = count;
                } else {
                    badge.style.display = 'none';
                }
            });
    }

    // Toggle dropdown open/close
    function toggleNotifications() {
        var dropdown = document.getElementById('notifDropdown');
        dropdown.classList.toggle('show');

        if (dropdown.classList.contains('show')) {
            loadNotifications();
        }
    }

    // Load all notifications into dropdown
    function loadNotifications() {
        fetch('/Notifications/GetAll')
            .then(r => r.json())
            .then(data => {
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
                        + '<button class="notif-delete-btn" onclick="deleteNotification(' + n.id + ')">✕</button>'
                        + '</div>';
                });

                list.innerHTML = html;
            });
    }

    // Delete a single notification by ID
    function deleteNotification(id) {
        fetch('/Notifications/Delete?id=' + id, { method: 'POST' })
            .then(() => {
                // Remove the item from DOM without reloading the whole list
                var item = document.getElementById('notif-' + id);
                if (item) item.remove();

                // If list is now empty, show the empty message
                var list = document.getElementById('notifList');
                if (list.children.length === 0) {
                    list.innerHTML = '<div class="notif-empty">No notifications yet.</div>';
                }

                loadUnreadCount();
            });
    }

    // Mark all as read
    function markAllRead() {
        fetch('/Notifications/MarkAllRead', { method: 'POST' })
            .then(() => {
                loadUnreadCount();
                loadNotifications();
            });
    }

    // Show a small toast popup in the bottom-right corner
    function showToast(message) {
        var toast = document.createElement('div');
        toast.className = 'notif-toast';
        toast.innerHTML = '<strong>🔔 New Complaint</strong><div style="margin-top:4px;font-size:12px;">' + message + '</div>';
        document.body.appendChild(toast);

        // Auto-remove after 4 seconds
        setTimeout(function () {
            toast.remove();
        }, 4000);
    }

    // Close dropdown when clicking outside
    document.addEventListener('click', function (e) {
        var dropdown = document.getElementById('notifDropdown');
        var bell = document.getElementById('bellBtn');
        if (!bell.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.remove('show');
        }
    });

}