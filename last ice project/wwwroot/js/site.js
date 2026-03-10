const uri = '/IceCream';
let pizzas = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const item = {
        isGlutenFree: false,
        name: addNameTextbox.value.trim()
    };

    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Name: item.name, IsGlutenFree: item.isGlutenFree })
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to add');
            return response.json().catch(() => {});
        })
        .then(() => {
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE'
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = pizzas.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-isGlutenFree').checked = item.isGlutenFree;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        IsGlutenFree: document.getElementById('edit-isGlutenFree').checked,
        Name: document.getElementById('edit-name').value.trim()
    };

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'iceCream' : 'iceCream kinds';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const grid = document.getElementById('iceCreamGrid');
    grid.innerHTML = '';

    _displayCount(data.length);

    data.forEach(item => {
        const card = document.createElement('div');
        card.className = 'card';

        const title = document.createElement('h4');
        title.innerText = item.name || item.Name;
        card.appendChild(title);

        const desc = document.createElement('p');
        desc.innerText = item.isGlutenFree ? 'Gluten free' : 'Contains gluten';
        card.appendChild(desc);

        const actions = document.createElement('div');
        actions.className = 'actions';

        const editBtn = document.createElement('button');
        editBtn.className = 'button ghost';
        editBtn.innerText = 'Edit';
        editBtn.onclick = () => displayEditForm(item.id || item.Id);

        const delBtn = document.createElement('button');
        delBtn.className = 'button';
        delBtn.innerText = 'Delete';
        delBtn.onclick = () => deleteItem(item.id || item.Id);

        actions.appendChild(editBtn);
        actions.appendChild(delBtn);
        card.appendChild(actions);

        grid.appendChild(card);
    });

    pizzas = data;
}