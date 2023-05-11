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
    <>
      <a className="test-class-a"
        href={aProfile.replace(/utm_source=[^&]+/, `utm_source=${appName}`)}>
        {name}
      </a> 
      on 
      <a className="test-class-a"
        href={aUnsplash.replace(/utm_source=[^&]+/, `utm_source=${appName}`)}>
        Unsplash
      </a>
    </>
  )
}