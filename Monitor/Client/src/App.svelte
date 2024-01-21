<script lang="ts">
  import Dashboard from './lib/Dashboard.svelte';
  const serverUrl = "http://127.0.0.1";
  
</script>

<main>
  <Dashboard src={serverUrl}></Dashboard>
</main>

<style>
</style>
