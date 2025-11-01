<template>
  <div id="layout">
    <header>
      <div class="wrapper">
        <h1>Sistema de Nóminas</h1>
        <nav>
          <router-link to="/empleados">Empleados</router-link>
          <router-link to="/departamentos">Departamentos</router-link>
          <router-link to="/puestos">Puestos</router-link>
          <router-link to="/ingresos">Tipos de Ingresos</router-link>
          <router-link to="/deducciones">Deducciones</router-link>
        </nav>
      </div>
    </header>

    <main>
      <router-view />
    </main>
  </div>
</template>

<style scoped>
  #layout {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  header {
    line-height: 1.5;
    background-color: var(--color-background-mute);
    border-bottom: 1px solid var(--color-border);
    padding: 1rem 2rem;
  }

  .wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 0 auto;
  }

  h1 {
    font-weight: bold;
    font-size: 1.5rem;
    color: var(--color-heading);
  }

  nav {
    display: flex;
    gap: 1rem;
  }

  nav a {
    font-size: 1rem;
    color: var(--color-text);
    text-decoration: none;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
  }

  nav a.router-link-exact-active {
    color: var(--color-background);
    background-color: hsla(160, 100%, 37%, 1);
  }

  nav a:hover {
    background-color: var(--color-border);
  }

  main {
    flex-grow: 1;
    padding: 2rem;
    margin: 0 auto;
    width: 100%;
  }
</style>
