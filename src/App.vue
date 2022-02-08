<template>
  <div>Hello {{ value }}</div>
</template>

<script>
export default {
  name: "App",
  data() {
    return {
      value: []
    };
  },
  async mounted() {
    const { scores } = await (await fetch("/api/GetEntries")).json();
    this.value = scores;
  }
};
</script>
