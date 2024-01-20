/**
 * @param {string} [serverAddress]
 * @param {string} ipAddress
 */
export async function getLatestImage(serverAddress, ipAddress) {
    let url = serverAddress + "/api/image?ipAddress=" + ipAddress;

    return fetch(url)
        .then(response => response.json(), error => error)
}