<template>
  <div>
    <header class="site-header sticky-top py-1">
      <nav
        class="container d-flex flex-column flex-md-row justify-content-between"
      >
        <a class="py-2" href="#" aria-label="Product">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            fill="none"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            class="d-block mx-auto"
            role="img"
            viewBox="0 0 24 24"
          >
            <title>Product</title>
            <circle cx="12" cy="12" r="10" />
            <path
              d="M14.31 8l5.74 9.94M9.69 8h11.48M7.38 12l5.74-9.94M9.69 16L3.95 6.06M14.31 16H2.83m13.79-4l-5.74 9.94"
            />
          </svg>
        </a>
        <a class="py-2 d-none d-md-inline-block" href="#">Product</a>
        <a class="py-2 d-none d-md-inline-block" href="#">Features</a>
        <a class="py-2 d-none d-md-inline-block" href="#">Pricing</a>
      </nav>
    </header>

    <main>
      <div
        class="
          position-relative
          overflow-hidden
          p-3 p-md-5
          m-md-3
          text-center
          bg-light
        "
      >
        <div class="col-md-5 p-lg-5 mx-auto my-5">
          <h1 class="display-4 fw-normal">Scoreboards Inc</h1>
          <p class="lead fw-normal">
            Need somewhere to put your point data and get it back in order? We
            got you covered.
          </p>
          <p>Go ahead, create a scoreboard.</p>
          <div class="input-group mb-3">
            <div class="input-group-prepend">
              <span class="input-group-text" id="scoreboard-name">
                Scoreboard
              </span>
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
          <button class="btn btn-outline-secondary" @click="submit">
            Register
          </button>
          <div v-show="showToken">
            <p>This is your scoreboard token, guard it with your life.</p>
            <h3>{{ token }}</h3>
          </div>
        </div>
        <div class="product-device shadow-sm d-none d-md-block"></div>
        <div
          class="product-device product-device-2 shadow-sm d-none d-md-block"
        ></div>
      </div>
    </main>

    <footer class="container py-5">
      <div class="row">
        <div class="col-12 col-md">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            fill="none"
            stroke="currentColor"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            class="d-block mb-2"
            role="img"
            viewBox="0 0 24 24"
          >
            <title>Product</title>
            <circle cx="12" cy="12" r="10" />
            <path
              d="M14.31 8l5.74 9.94M9.69 8h11.48M7.38 12l5.74-9.94M9.69 16L3.95 6.06M14.31 16H2.83m13.79-4l-5.74 9.94"
            />
          </svg>
          <small class="d-block mb-3 text-muted">&copy; 2017–2021</small>
        </div>
        <div class="col-6 col-md">
          <h5>Features</h5>
          <ul class="list-unstyled text-small">
            <li><a class="link-secondary" href="#">Cool stuff</a></li>
            <li><a class="link-secondary" href="#">Random feature</a></li>
            <li><a class="link-secondary" href="#">Team feature</a></li>
            <li><a class="link-secondary" href="#">Stuff for developers</a></li>
            <li><a class="link-secondary" href="#">Another one</a></li>
            <li><a class="link-secondary" href="#">Last time</a></li>
          </ul>
        </div>
        <div class="col-6 col-md">
          <h5>Resources</h5>
          <ul class="list-unstyled text-small">
            <li><a class="link-secondary" href="#">Resource name</a></li>
            <li><a class="link-secondary" href="#">Resource</a></li>
            <li><a class="link-secondary" href="#">Another resource</a></li>
            <li><a class="link-secondary" href="#">Final resource</a></li>
          </ul>
        </div>
        <div class="col-6 col-md">
          <h5>About</h5>
          <ul class="list-unstyled text-small">
            <li><a class="link-secondary" href="#">Team</a></li>
            <li><a class="link-secondary" href="#">Locations</a></li>
            <li><a class="link-secondary" href="#">Privacy</a></li>
            <li><a class="link-secondary" href="#">Terms</a></li>
          </ul>
        </div>
      </div>
    </footer>
  </div>
</template>

<script>
export default {
  name: "App",
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
        })
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

<style>
.container {
  max-width: 960px;
}

/*
 * Custom translucent site header
 */

.site-header {
  background-color: rgba(0, 0, 0, 0.85);
  -webkit-backdrop-filter: saturate(180%) blur(20px);
  backdrop-filter: saturate(180%) blur(20px);
}
.site-header a {
  color: #8e8e8e;
  transition: color 0.15s ease-in-out;
}
.site-header a:hover {
  color: #fff;
  text-decoration: none;
}

/*
 * Dummy devices (replace them with your own or something else entirely!)
 */

.product-device {
  position: absolute;
  right: 10%;
  bottom: -30%;
  width: 300px;
  height: 540px;
  background-color: #333;
  border-radius: 21px;
  transform: rotate(30deg);
}

.product-device::before {
  position: absolute;
  top: 10%;
  right: 10px;
  bottom: 10%;
  left: 10px;
  content: "";
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 5px;
}

.product-device-2 {
  top: -25%;
  right: auto;
  bottom: 0;
  left: 5%;
  background-color: #e5e5e5;
}

.bd-placeholder-img {
  font-size: 1.125rem;
  text-anchor: middle;
  -webkit-user-select: none;
  -moz-user-select: none;
  user-select: none;
}

@media (min-width: 768px) {
  .bd-placeholder-img-lg {
    font-size: 3.5rem;
  }
}
</style>