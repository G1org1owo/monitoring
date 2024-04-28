import { readable } from 'svelte/store';
import { getLatestImage, getConnectedClients } from './apiClient';



/**
 * @param {string} targetUsername
 */
export function createImage(targetUsername) {
    return readable({error: true, info: "Not initialized", imageUrl: "", timestamp: ""}, function start(set) {
        const interval = setInterval(() => {
            getLatestImage(targetUsername)
                .then(image => set(image))
        }, 1000);

        return function stop() {
            clearInterval(interval);
        }
    });
}

export function createClientsStore() {
    return readable([], function start(set) {
        const interval = setInterval(() => {
            getConnectedClients()
                .then(clients => set(clients));
    }, 5000);

        return function stop() {
            clearInterval(interval);
        }
    })
}