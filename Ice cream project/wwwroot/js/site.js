const uri = '/IceCream';
const tokenKey = 'icecream_token';
let authToken = localStorage.getItem(tokenKey) || '';
let iceCreams = [];

// --- עזרים לטוקן ואבטחה ---
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

function getAuthHeaders(){
    const headers = {};
    if (authToken) headers['Authorization'] = `Bearer ${authToken}`;
    return headers;
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

function closeInput() { 
    document.getElementById('editForm').style.display = 'none'; 
}



function getItems() {
    fetch(uri, { headers: getAuthHeaders() })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(data => {
            _displayItems(data || []);
        })
        .catch(error => {
            console.error('Unable to get items.', error);
            _displayItems([]); 
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
        showToast('Item added successfully');
        addNameTextbox.value = '';
     
    })
    .catch(error => console.error('Error adding item:', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: getAuthHeaders()
    })
    .then(response => {
        if (!response.ok) throw new Error('Delete failed');
        showToast('Item deleted successfully');
     
    })
    .catch(error => console.error('Error deleting item:', error));
}

function updateItem() {
    const itemId = parseInt(document.getElementById('edit-id').value, 10);
    const item = {
        Id: itemId,
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
        showToast('Item updated successfully');
        closeInput();
        
    })
    .catch(error => console.error('Error updating item:', error));

    return false;
}

function displayEditForm(id) {
    const item = iceCreams.find(i => i.Id === id);
    if (!item) return;
    document.getElementById('edit-name').value = item.Name;
    document.getElementById('edit-id').value = item.Id;
    document.getElementById('edit-isGlutenFree').checked = item.IsGlutenFree;
    document.getElementById('editForm').style.display = 'block';
}


function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'iceCream' : 'iceCream kinds';
    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    iceCreams = data.map(d => ({ 
        Id: d.Id || d.id, 
        Name: d.Name || d.name, 
        IsGlutenFree: (d.IsGlutenFree !== undefined) ? d.IsGlutenFree : d.isGlutenFree 
    }));

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
        if (activityList) {
            const li = document.createElement("li");
            li.textContent = `${username} ${action} '${IceCreamName}'`;
            activityList.insertBefore(li, activityList.firstChild);

            while (activityList.children.length > 10) {
                activityList.removeChild(activityList.lastChild);
            }
        }
        
    
        getItems();
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR connection error:", err));
}

document.addEventListener('DOMContentLoaded', function () {
    if (isTokenValid(authToken)) {
        getItems();
        initSignalR();
    }
    toggleAdminLinks();
});