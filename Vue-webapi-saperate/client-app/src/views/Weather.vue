<template>
  <div class="weather">
    <h1>This is an Weather page</h1>
    <Forecast :forCastFor="forCastFor"/>
  </div>
</template>

<script >
  import Forecast from '@/components/Forecast.vue'
  export default {
    name: 'Weather',
    data() {
      return {
        forCastFor:"Weather"
      }
    },
    components: {
      Forecast
    }
  }
</script>
