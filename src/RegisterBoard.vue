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
    <button class="btn btn-outline-secondary mb-3" @click="submit" :disabled="isDisabled">
      Register
    </button>
    <div v-show="showToken" class="alert alert-success" role="alert">
      <p>This is your scoreboard token, guard it with your life.</p>
      <h5>{{ token }}</h5>
    </div>
    <div v-show="showConflict" class="alert alert-danger" role="alert">
      Sorry, a board with that name seems to already exist. Please choose
      another name.
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
      showConflict: false,
    };
  },
  computed: {
      isDisabled() {
          let validRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;
          let isBoardNameSet = !(!this.boardName || this.boardName.length === 0)
          let isEmailSet = this.email.match(validRegex);
          return !(isBoardNameSet && isEmailSet);
      }
  },
  methods: {
    async submit() {
      this.showConflict = false;
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

      if (result.ok) {
        console.log("success");
        let data = await result.json();
        this.token = data.token;
        this.name = data.name;
        this.boardName = "";
        this.email = "";
        this.showToken = true;
      } else {
        console.log("failed");
        this.showToken = false;
        this.token = "";
        this.name = "";
        if (result.status == 409) this.showConflict = true;
      }
    },
  },
};
</script>