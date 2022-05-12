<template>
  <div class="col-md-5 p-lg-5 mx-auto my-5">
    <h1 class="display-4 fw-normal">Scoreboards Inc</h1>
    <p class="lead fw-normal">
      Need somewhere to put your point data and get it back in order? We got you
      covered.
    </p>
    <p>Go ahead, create a scoreboard.</p>
    <RegisterBoard />
  </div>
</template>

<script>
import RegisterBoard from './RegisterBoard.vue';


export default {    
  name: "HomeRegister",
  components: {
    RegisterBoard
  },
};
</script>
