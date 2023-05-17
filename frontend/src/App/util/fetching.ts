import axios from 'axios';

const dev_url = "http://localhost:5001/api"
const dev_mode = true

export const fetching = async (enpoint: string) => {
  if (dev_mode) {
    const optionObj = { 
      headers : { 
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    }
  
    return fetch(`./mock/${enpoint}.data.json`, optionObj)
      .then(response => response.json())
  }

  return axios.get(`${dev_url}/${enpoint}`)
    .then(res => res.data)
}