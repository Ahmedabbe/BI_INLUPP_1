let tabledata = document.getElementById('tabledata')

fetch("https://iot20ahmed.azurewebsites.net/api/GetAllDataFromCosmosDB?")
.then(res => res.json())
.then(data => {
    for(let row of data) {
        tabledata.innerHTML += `<tr><td>${row.Id}</td><td>${row.deviceId}</td><td>${row.ts}</td><td>${row.temp}</td><td>${row.hum}</td>`
    }
})