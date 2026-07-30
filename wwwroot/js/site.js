var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.start().catch(function (err) {
    console.error("SignalR error: " + err.toString());
});

// Live table updates
function getStatusBadge(status) {
    if (status === "Pending") return '<span class="badge bg-warning text-dark">Pending</span>';
    if (status === "InProgress") return '<span class="badge bg-primary">In Progress</span>';
    if (status === "Resolved") return '<span class="badge bg-success">Resolved</span>';
    if (status === "Rejected") return '<span class="badge bg-danger">Rejected</span>';
    return '<span class="badge bg-secondary">' + status + '</span>';
}

connection.on("ComplaintStatusUpdated", function (id, newStatus, adminNote) {
    var statusCell = document.getElementById("status-" + id);
    if (statusCell) statusCell.innerHTML = getStatusBadge(newStatus);

    var editBtn = document.getElementById("editbtn-" + id);
    if (editBtn && newStatus !== "Pending") editBtn.style.display = "none";
});

connection.on("ComplaintDeleted", function (id) {
    var row = document.getElementById("row-" + id);
    if (row) row.remove();
});

connection.on("ComplaintEdited", function (id, newTitle, newCategory) {
    var titleCell = document.getElementById("title-" + id);
    if (titleCell) titleCell.innerText = newTitle;

    var categoryCell = document.getElementById("category-" + id);
    if (categoryCell) categoryCell.innerText = newCategory;
});

connection.on("NewComplaintRow", function (c) {
    var tbody = document.getElementById("complaintsTableBody");
    if (!tbody) return;

    var row = document.createElement("tr");
    row.id = "row-" + c.id;
    row.innerHTML =
        '<td>' + c.id + '</td>' +
        '<td id="title-' + c.id + '">' + c.title + '</td>' +
        '<td id="category-' + c.id + '">' + c.category + '</td>' +
        '<td>' + c.submittedBy + '</td>' +
        '<td>' + c.datetime + '</td>' +
        '<td id="status-' + c.id + '"><span class="badge bg-warning text-dark">Pending</span></td>' +
        '<td>' +
        '<a href="/Complaints/Details/' + c.id + '" class="btn btn-sm btn-outline-primary">Details</a>' +
        ' <a href="/Complaints/Edit/' + c.id + '" class="btn btn-sm btn-warning ms-1">Edit</a>' +
        ' <form action="/Complaints/Delete/' + c.id + '" method="post" style="display:inline">' +
        '<button type="submit" class="btn btn-sm btn-danger ms-1" onclick="return confirm(\'Delete?\')">Delete</button>' +
        '</form>' +
        '</td>';

    tbody.appendChild(row);
});

// ─── Bell notifications (Admin only) ───────────────────────────────────────
var bellBtn = document.getElementById('bellBtn');

if (bellBtn) {

    loadUnreadCount();

    connection.on("ReceiveNotification", function (message) {
        loadUnreadCount();
        showToast(message);
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
                    var time = new Date(n.createdOn).toLocaleString();

                    html += '<div class="notif-item ' + cssClass + '" id="notif-' + n.id + '">'
                        + '<div class="notif-body">'
                        + n.message
                        + '<div class="notif-time">' + time + '</div>'
                        + '</div>'
                        + '<button onclick="deleteNotification(' + n.id + ')" class="notif-delete-btn">&#10005;</button>'
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
        var existing = document.getElementById('liveToast');
        if (existing) existing.remove();

        var toast = document.createElement('div');
        toast.id = 'liveToast';
        toast.className = 'notif-toast';
        toast.innerHTML =
            '<div style="display:flex;justify-content:space-between;align-items:flex-start;gap:10px;">'
            + '<div>'
            + '<strong>🔔 New Complaint</strong>'
            + '<div style="margin-top:20px;font-size:20px;">' + message + '</div>'
            + '</div>'
            + '<button onclick="document.getElementById(\'liveToast\').remove()" '
            + 'style="background:none;border:none;color:white;font-size:36px;cursor:pointer;line-height:1;padding:0;">&#10005;</button>'
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