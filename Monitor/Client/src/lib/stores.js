import { readable } from 'svelte/store';
import { getLatestImage } from './apiClient';



/**
 * @param {string} serverAddress
 * @param {string} targetAddress
 */
export function createImage(serverAddress, targetAddress) {
    return readable({error: true, info: "Not initialized", imageUrl: "", timestamp: ""}, function start(set) {
        const interval = setInterval(() => {
            getLatestImage(serverAddress, targetAddress)
                .then(image => set(image))
        }, 1000);


        return function stop() {
            clearInterval(interval);
        }
    });
}