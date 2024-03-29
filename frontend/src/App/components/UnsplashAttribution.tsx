import '../styles/components/unsplash-attribution.css'

export const UnsplashAttribution = (
  {aProfile, name, aUnsplash}: {
    aProfile: string
    name: string
    aUnsplash: string
  }
) => {
  const appName = "thrifty_helper"

  return (
    <p className='us-attribution-p'>
      Image by 
      <a className="us-attribution-link"
        href={aProfile.replace(/utm_source=[^&]+/, `utm_source=${appName}`)}>
        {name}
      </a> 
      on 
      <a className="us-attribution-link"
        href={aUnsplash.replace(/utm_source=[^&]+/, `utm_source=${appName}`)}>
        Unsplash
      </a>
    </p>
  )
}