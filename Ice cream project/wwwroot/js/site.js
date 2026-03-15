const uri = '/IceCream';
const tokenKey = 'icecream_token';
let authToken = localStorage.getItem(tokenKey) || '';
let iceCreams = [];

function parseJwt(token) {
    try {
        const payload = token.split('.')[1];
        const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
        return JSON.parse(decodeURIComponent(escape(decoded)));
    } catch (e) { return null; }
}

function isTokenValid(token) {
    if (!token) return false;
    const payload = parseJwt(token);
    if (!payload || !payload.exp) return false;
    const exp = payload.exp * 1000;
    return Date.now() < exp;
}

// redirect to login if no valid token
if (!isTokenValid(authToken)) {
    localStorage.removeItem(tokenKey);
    authToken = '';
    if (!location.pathname.endsWith('login.html')) {
        window.location.href = '/login.html';
    }
}

function isAdminFromToken() {
    const payload = parseJwt(authToken);
    return payload && (payload.type === 'Admin' || payload.type === 'admin');
}

function showToast(msg) {
    let t = document.getElementById('toast');
    if (!t) {
        t = document.createElement('div');
        t.id = 'toast';
        t.className = 'toast';
        document.body.appendChild(t);
    }
    t.innerText = msg;
    t.style.opacity = 1;
    setTimeout(() => { t.style.opacity = 0; }, 3000);
}

function toggleAdminLinks() {
    const usersLink = document.getElementById('usersLink');
    if (usersLink) usersLink.style.display = isAdminFromToken() ? 'inline' : 'none';
}

// ensure admin link visibility at startup
document.addEventListener('DOMContentLoaded', toggleAdminLinks);

const fallbackData = [
    { Id: 1, Name: 'chocolate', IsGlutenFree: true },
    { Id: 2, Name: 'Pistachio', IsGlutenFree: false },
    { Id: 3, Name: 'hghg', IsGlutenFree: false },
    { Id: 4, Name: 'hgrfg', IsGlutenFree: false },
    { Id: 5, Name: 'fgf', IsGlutenFree: false }
];

function getAuthHeaders(){
    const headers = {};
    if (authToken) headers['Authorization'] = `Bearer ${authToken}`;
    return headers;
}

function getItems() {
    fetch(uri, { headers: getAuthHeaders() })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json().catch(() => null);
        })
        .then(data => {
            if (!data || (Array.isArray(data) && data.length === 0)) {
                _displayItems(fallbackData);
            } else {
                _displayItems(data);
            }
        })
        .catch(error => {
            console.error('Unable to get items.', error);
            _displayItems(fallbackData);
        });
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const name = addNameTextbox.value.trim();
    if (!name) return;
     const itemPayload = { Name: name, IsGlutenFree: false };

    fetch(uri, {
            method: 'POST',
            headers: Object.assign({ 'Accept': 'application/json', 'Content-Type': 'application/json' }, getAuthHeaders()),
                body: JSON.stringify(itemPayload)
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to add');
            return response.json().catch(() => null);
        })
         .then(() => {
            // Do not refresh immediately; rely on SignalR notification to update the list
            showToast('Item added successfully');
            addNameTextbox.value = '';
        })
          .catch(error => {
            console.warn('Unable to add item to server, adding locally.', error);
            const maxId = iceCreams.length ? Math.max(...iceCreams.map(p => p.Id || p.id)) : 0;
            const newItem = { Id: maxId + 1, Name: name, IsGlutenFree: false };
            iceCreams.push(newItem);
            _displayItems(iceCreams);
            addNameTextbox.value = '';
        });
}

function deleteItem(id) {
     fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        })
        .then(() => {
            showToast('Item deleted successfully');
        })
        .catch(error => {
            console.error('Unable to delete item.', error);
            showToast('Delete failed');
        });
}

function displayEditForm(id) {
     const item = iceCreams.find(i => (i.id || i.Id) === id);
    if (!item) return;
      document.getElementById('edit-name').value = item.Name || item.name || '';
    document.getElementById('edit-id').value = item.Id || item.id;
    document.getElementById('edit-isGlutenFree').checked = (item.IsGlutenFree !== undefined) ? item.IsGlutenFree : item.isGlutenFree;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
     const itemId = parseInt(document.getElementById('edit-id').value, 10);
    const item = {
            Id: parseInt(itemId, 10),
        Name: document.getElementById('edit-name').value.trim(),
        IsGlutenFree: document.getElementById('edit-isGlutenFree').checked
    };

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: Object.assign({ 'Accept': 'application/json', 'Content-Type': 'application/json' }, getAuthHeaders()),
            body: JSON.stringify(item)
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to update');
            return response;
        })
        .then(() => {
            showToast('Item updated successfully');
        })
        .catch(error => {
            console.warn('Unable to update on server, updating locally.', error);
            const idx = iceCreams.findIndex(p => (p.Id || p.id) === itemId);
            if (idx !== -1) iceCreams[idx] = item;
            _displayItems(iceCreams);
        });

    closeInput();

    return false;
}
function closeInput() { document.getElementById('editForm').style.display = 'none'; }

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'iceCream' : 'iceCream kinds';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
       // normalize incoming items to Id/Name/IsGlutenFree
    iceCreams = data.map(d => ({ Id: d.Id || d.id, Name: d.Name || d.name, IsGlutenFree: (d.IsGlutenFree !== undefined) ? d.IsGlutenFree : d.isGlutenFree }));
    const grid = document.getElementById('iceCreamGrid');
    grid.innerHTML = '';
    _displayCount(iceCreams.length);
    iceCreams.forEach(item => {
        const card = document.createElement('div');
        card.className = 'card';
        const title = document.createElement('h4');
        title.innerText = item.Name;
        card.appendChild(title);
        const desc = document.createElement('p');
        desc.innerText = item.IsGlutenFree ? 'Gluten free' : 'Contains gluten';
        card.appendChild(desc);
        const actions = document.createElement('div');
        actions.className = 'actions';
        const editBtn = document.createElement('button');
        editBtn.className = 'button ghost';
        editBtn.innerText = 'Edit';
        editBtn.onclick = () => displayEditForm(item.Id);
        const delBtn = document.createElement('button');
        delBtn.className = 'button';
        delBtn.innerText = 'Delete';
        delBtn.onclick = () => deleteItem(item.Id);
        actions.appendChild(editBtn);
        actions.appendChild(delBtn);
        card.appendChild(actions);
        grid.appendChild(card);
    });
}
function initSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/activityHub", { accessTokenFactory: () => authToken })
        .build();

    connection.on("ReceiveActivity", function (username, action, IceCreamName) {
        const activityList = document.getElementById("activityList");
        const li = document.createElement("li");
        li.textContent = `${username} ${action} '${IceCreamName}'`;
        activityList.insertBefore(li, activityList.firstChild);

        // Keep only last 10 activities
        while (activityList.children.length > 10) {
            activityList.removeChild(activityList.lastChild);
        }
        // When signalr notifies, refresh list from server
        try { getItems(); } catch (e) { console.error('SignalR update failed to refresh items', e); }
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR connection error:", err));
}

// initialize on load
document.addEventListener('DOMContentLoaded', function () {
    if (isTokenValid(authToken)) {
        getItems();
        initSignalR();
    }
    toggleAdminLinks();
});