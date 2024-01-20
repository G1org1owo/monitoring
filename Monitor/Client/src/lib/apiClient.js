/**
 * @param {string} [url]
 */
export async function getLatestImage(url) {
    return fetch(url)
        .then(response => response.json(), error => error)
}