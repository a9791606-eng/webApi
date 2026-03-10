const uri = '/IceCream';
let pizzas = [];

const fallbackData = [
    { Id: 1, Name: 'chocolate', IsGlutenFree: true },
    { Id: 2, Name: 'Pistachio', IsGlutenFree: false },
    { Id: 3, Name: 'hghg', IsGlutenFree: false },
    { Id: 4, Name: 'hgrfg', IsGlutenFree: false },
    { Id: 5, Name: 'fgf', IsGlutenFree: false }
];

function getItems() {
    fetch(uri)
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(data => _displayItems(data))
        .catch(error => {
            console.warn('Unable to get items from server, using fallback data.', error);
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
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(itemPayload)
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to add');
            return response.json().catch(() => null);
        })
        .then(created => {
            if (created) {
                // server returned created item
                getItems();
            } else {
                // server didn't return body (or unreachable) — update locally
                const maxId = pizzas.length ? Math.max(...pizzas.map(p => p.Id || p.id)) : 0;
                const newItem = { Id: maxId + 1, Name: name, IsGlutenFree: false };
                pizzas.push(newItem);
                _displayItems(pizzas);
            }
            addNameTextbox.value = '';
        })
        .catch(error => {
            console.warn('Unable to add item to server, adding locally.', error);
            const maxId = pizzas.length ? Math.max(...pizzas.map(p => p.Id || p.id)) : 0;
            const newItem = { Id: maxId + 1, Name: name, IsGlutenFree: false };
            pizzas.push(newItem);
            _displayItems(pizzas);
            addNameTextbox.value = '';
        });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, { method: 'DELETE' })
        .then(response => {
            if (!response.ok) throw new Error('Failed to delete');
            return response;
        })
        .then(() => getItems())
        .catch(error => {
            console.warn('Unable to delete on server, removing locally.', error);
            pizzas = pizzas.filter(p => (p.Id || p.id) !== id);
            _displayItems(pizzas);
        });
}

function displayEditForm(id) {
    const item = pizzas.find(it => (it.id || it.Id) === id);
    if (!item) return;

    document.getElementById('edit-name').value = item.Name || item.name || '';
    document.getElementById('edit-id').value = item.Id || item.id;
    document.getElementById('edit-isGlutenFree').checked = (item.IsGlutenFree !== undefined) ? item.IsGlutenFree : item.isGlutenFree;
    document.getElementById('editForm').style.display = 'block';
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
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to update');
            return response;
        })
        .then(() => getItems())
        .catch(error => {
            console.warn('Unable to update on server, updating locally.', error);
            const idx = pizzas.findIndex(p => (p.Id || p.id) === itemId);
            if (idx !== -1) pizzas[idx] = item;
            _displayItems(pizzas);
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
    pizzas = data.map(d => ({ Id: d.Id || d.id, Name: d.Name || d.name, IsGlutenFree: (d.IsGlutenFree !== undefined) ? d.IsGlutenFree : d.isGlutenFree }));

    const grid = document.getElementById('iceCreamGrid');
    grid.innerHTML = '';

    _displayCount(pizzas.length);

    pizzas.forEach(item => {
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