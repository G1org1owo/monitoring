/**
 * @param {string} ipAddress
 */
export async function getLatestImage(ipAddress) {
    let url ="/api/image?ipAddress=" + ipAddress;

    return fetch(url)
        .then(response => response.json(), error => error)
}

export async function getConnectedClients() {
    let url = "/api/clients";

    return fetch(url)
        .then(response => response.json(), error => error);
}