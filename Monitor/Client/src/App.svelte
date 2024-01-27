<script lang="ts">
  import Dashboard from './lib/Dashboard.svelte';
  import Nav from './Nav.svelte';
</script>

<Nav></Nav>
<main>
  <Dashboard></Dashboard>
</main>

<style>
  main {
    display: flex;
    justify-content: center;
    align-items: center;
    margin: 0;
    width: 95%;
    height: 95%;
  }
</style>
