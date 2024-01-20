<script lang="ts">
  import Screen from "./lib/Screen.svelte";
  import { createClientsStore } from "./lib/stores";

  const serverUrl = "http://127.0.0.1";
  const clients = createClientsStore("")
</script>

<main>
  {#each $clients as client}
    <Screen on:connectionLost={() => console.log("connection lost")}
      src={serverUrl}
      target={client}>
    
    </Screen>
  {/each}
</main>

<style>
</style>
