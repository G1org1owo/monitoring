<script lang="ts">
  import Screen from "./lib/Screen.svelte";

  let enabled = true;
  let ipAddress = '127.0.0.1';
</script>

<main>
  {#if enabled}
    <Screen on:connectionLost={() => console.log("connection lost")}
      src="http://127.0.0.1"
      target={ipAddress}>
    
    </Screen>
  {/if}
</main>

<button on:click={() => enabled = !enabled}>Update screen</button>

<style>
</style>
