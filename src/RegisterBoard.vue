<template>
  <div>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text" id="scoreboard-name"> Scoreboard </span>
      </div>
      <input
        v-model="boardName"
        type="text"
        class="form-control"
        placeholder="Name"
        aria-label="ScoreboardName"
        aria-describedby="scoreboard-name"
      />
    </div>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text" id="email">@</span>
      </div>
      <input
        v-model="email"
        type="text"
        class="form-control"
        placeholder="your@user.com"
        aria-label="Email"
        aria-describedby="email"
      />
    </div>
    <button class="btn btn-outline-secondary" @click="submit">Register</button>
    <div v-show="showToken">
      <p>This is your scoreboard token, guard it with your life.</p>
      <h3>{{ token }}</h3>
    </div>
  </div>
</template>

<script>
export default {
  name: "RegisterBoard",
  data() {
    return {
      boardName: "",
      name: "",
      email: "",
      token: "",
      showToken: false,
    };
  },
  methods: {
    async submit() {
      const result = await fetch(`/api/createboard`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          boardName: this.boardName,
          email: this.email,
        }),
      });
      console.log(result);
      let data = await result.json();

      if (result.ok) {
        this.token = data.token;
        this.name = data.name;
        this.boardName = "";
        this.email = "";
        this.showToken = true;
      } else {
        this.showToken = false;
        this.token = "";
        this.name = "";
        console.log(data);
      }
    },
  },
};
</script>