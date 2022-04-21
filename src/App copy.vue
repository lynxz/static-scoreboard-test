<template>
  <div class="score-board">
    <h1>High Scores</h1>
    <ul  class="rank-list">
      <li v-for="(score, index) in scores" :key="`score-${index}`">
        <div class="rank-row">
          <div class="rank-no">
            <span>Rank</span>
            <h3>{{ index + 1 }}</h3>
          </div>
          <div class="rank-info">
            <h5>{{ score.userName }}</h5>
            <span>Date Submitted:</span>
            <span>{{ score.date | formatDate }}</span>
          </div>
          <div class="rank-points">
            <span>Points</span>
            <h3>{{ score.score }}</h3>
          </div>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "App-copy",
  data() {
    return {
      scores: [],
    };
  },
  async mounted() {
    const items = await (await fetch("/api/GetEntries")).json();
    this.scores = items;
  },
};
</script>

<style>
body {
  background-color: lightgray;
}

.score-board {
  margin-left: 20px;
}

.rank-list {
  width: 600px;
  background: white;
  padding: 20px 20px 20px 20px;
  font-weight: 400;
  border: 1px solid black;
  border-radius: 25px;
  list-style-type: none;
}

.rank-row {
  clear: both;
  padding-bottom: 20px;
  margin-bottom: 50px;
}

.rank-no {
  float: left;
  width: 80px;
  padding-top: 10px;
  padding-bottom: 10px;
  text-align: center;
  background-color: lightblue;
  border-left: 6px solid darkblue;
  color: darkblue;
  justify-content: center;
}

.rank-no h3 {
  margin: 0;
}

.rank-info {
  float: left;
  padding: 5px 10px;
}

.rank-info h5 {
  font-size: 18px;
  margin-bottom: 5px;
  margin-top: 0px;
}

.rank-info span {
  margin-bottom: 2px;
}

.rank-points {
  float: right;
  text-align: right;
  padding: 10px;
}

.rank-points h3 {
  margin: 0px;
}
</style>
