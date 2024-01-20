/**
 * @param {string} [serverAddress]
 * @param {string} ipAddress
 */
export async function getLatestImage(serverAddress, ipAddress) {
    let url = serverAddress + "/api/image?ipAddress=" + ipAddress;

    return fetch(url)
        .then(response => response.json(), error => error)
}

/**
 * @param {string} serverAddress
 */
export async function getConnectedClients(serverAddress) {
    let url = serverAddress + "/api/clients";

    return fetch(url)
        .then(response => response.json(), error => error);
}