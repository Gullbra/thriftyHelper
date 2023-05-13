import axios from 'axios';

// const dev_url = "http://localhost:5001/api/Recipies/blodpudding%20med%20ägg%20och%20bacon"
const dev_url = "http://localhost:5001/api"

export const fetching = async (enpoint: string) => {

  return axios.get(`${dev_url}/${enpoint}`)
    .then(res => res.data)
    .catch(e => console.log(e.message))
}