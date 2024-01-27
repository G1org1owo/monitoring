<script>
    import Screen from "./Screen.svelte";
    import { createClientsStore } from "./stores";
    
    let size = 30;

    /**
     * @param {number} nClients
     */
    function getSizeFor(nClients) {        
        if(nClients <= 1) {
            return 90;
        }

        if(nClients <= 4) {
            return 45;
        }

        if(nClients <= 9) {
            return 30;
        }

        if(nClients <= 16) {
            return 22.5;
        }

        if(nClients <= 25) {
            return 18;
        }
    }

    const clients = createClientsStore()

    $: size = getSizeFor($clients.length);
</script>

<div class=container>
{#each $clients as client (client)}
    <Screen on:connectionLost={() => console.log("connection lost")}
        target={client} {size}
    >

    </Screen>
{/each}
</div>

<style>
    .container {
        display:flex;
        flex-wrap: wrap;
        gap: 2vmin 2vmin;
        align-content: center;
        justify-content: center;
        width: 90%;
        height: 90%;
    }
</style>