<template>
  <div class="forcast">
    <h2>This is {{forCastFor}} Forcast</h2>
    <ul>
      <li v-for="item in weatherdata">
        <h2>{{item.summary}}</h2>
        <h3>{{item.date.split("T")[0]}}</h3>
        <h3>{{item.temperatureC}}</h3>
        <h3>{{item.temperatureF}}</h3>
      </li>
    </ul>

  </div>
</template>

<script>
  import services from '@/services/api.services';
export default {
    name: 'Forcast',
    data() {
      return {
        weatherdata: []
      }
    },
    props: {
      forCastFor: {
        type: String,
        value:''
      }
    },
    mounted() {
      var that = this;
      services.GetWeatherForcast().then(result => {
        if (result.data != null) {
          that.weatherdata = result.data;
        }
      }).catch(error => {
        console.log(error);
      });
    }
}
</script>

