<script>
    import Screen from "./Screen.svelte";
    import { createClientsStore } from "./stores";
    
    let size = 30;

    /**
     * @param {number} nClients
     */
    function getSizeFor(nClients) {        
        if(nClients <= 1) {
            return 100;
        }

        if(nClients <= 4) {
            return 50;
        }
    }

    const clients = createClientsStore()

    $: size = getSizeFor($clients.length);
</script>

<div class=container>
{#each $clients as client}
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
        grid-gap: 10px;
        align-items: center;
        justify-content: center;
    }
</style>