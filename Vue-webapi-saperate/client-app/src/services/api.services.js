import axios from 'axios'
import apiUrls from '@/constants/api.constants'

export default {
  GetWeatherForcast: function () {
    return axios.get(apiUrls.WeatherForcast)
  }
}
