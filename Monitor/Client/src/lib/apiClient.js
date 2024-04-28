/**
 * @param {string} username
 */
export async function getLatestImage(username) {
    let url ="/api/image?username=" + username;

    return fetch(url)
        .then(response => response.json(), error => error)
}

export async function getConnectedClients() {
    let url = "/api/clients";

    return fetch(url)
        .then(response => response.json(), error => error);
}