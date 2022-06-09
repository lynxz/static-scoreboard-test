<template>
  <div class="col-md-5 p-lg-5 mx-auto my-5">
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
        <span class="input-group-text" id="token">Token</span>
      </div>
      <input
        v-model="token"
        type="text"
        class="form-control"
        placeholder="00000000-0000-0000-0000-000000000000"
        aria-label="Token"
        aria-describedby="token"
      />
    </div>
    <button class="btn btn-outline-secondary mb-3" @click="loadBoard">
      Load
    </button>
    <div v-show="loaded">
      <div class="input-group mb-3">
        <div class="input-group-prepend">
          <span class="input-group-text" id="email">@</span>
        </div>
        <input
          v-model="board.email"
          type="text"
          class="form-control"
          aria-label="Email"
          aria-describedby="email"
        />
      </div>
      <div class="input-group mb-3">
        <div class="input-group-prepend">
          <span class="input-group-text" id="noOfResults">Results</span>
        </div>
        <input
          v-model="board.numberOfEntries"
          type="number"
          class="form-control"
          aria-label="NoOfResults"
          aria-describedby="noOfResults"
        />
      </div>
      <button
        type="button"
        class="btn btn-outline-secondary mb-3"
        @click="saveBoard"
      >
        Save
      </button>
      <button
        type="button"
        class="btn btn-outline-danger mb-3"
        @click="deleteBoard"
      >
        Delete
      </button>
      <div v-show="stored" class="alert alert-success" role="alert">
        <p>Board successfully updated</p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "UpdateBoard",
  data() {
    return {
      token: "",
      boardName: "",
      board: {
        numberOfEntries: 0,
        email: "",
      },
      loaded: false,
      stored: false,
    };
  },
  methods: {
    async loadBoard() {
      this.loaded = false;
      this.stored = false;

      const result = await fetch(
        `/api/getboard/${this.boardName}/${this.token}`
      );

      if (result.ok) {
        let data = await result.json();
        this.board.email = data.email;
        this.board.numberOfEntries = data.numberOfEntries;
        console.log(data);
        this.loaded = true;
      } else {
        this.board.email = "";
        this.board.numberOfEntries = 0;
        console.log(result.status);
      }
    },
    async saveBoard() {
      this.stored = false;

      const result = await fetch(`api/editboard/${this.boardName}/${this.token}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(this.board),
      });

      if (result.ok) {
        this.stored = true;
      }
    },
    async deleteBoard() {
      this.stored = false;

      const result = await fetch(`api/deleteboard/${this.boardName}/${this.token}`, {
        method: "DELETE",
      });

      if (result.ok) 
        this.$router.push({ path: '/' });
    },
  },
};
</script>
