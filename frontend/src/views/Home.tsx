
import { UnsplashAttribution } from '../components/UnsplashAttribution'
import '../styles/views/home-view.css'

export const Home = () => {
  return (
    <div className='home-view__view-wrapper --dev-border'>
      <img className="home-view__image-wrapper__image" src="./assets/chad-montano-eeqbbemH9-c-unsplash.jpg" alt="pancakes and syrup" />

      <div className="home-view__greeting-wrapper">
        <h2 className="home-view__greeting-wrapper__title">title text here. two or three lines</h2>

        <div className="home-view__greeting-wrapper__btn-container">
          <button className="home-view__greeting-wrapper__nav-button">btn 1</button>
          <button className="home-view__greeting-wrapper__nav-button">btn 1</button>
          <button className="home-view__greeting-wrapper__nav-button">btn 1</button>
        </div>
      </div>

      <div className="home-view__image-wrapper --dev-border">
        <p className="home-view__image-wrapper__attribution-text">
          <UnsplashAttribution 
            aProfile={'https://unsplash.com/@briewilly?utm_source=your_app_name&utm_medium=referral'}
            name='Chad Montano'
            aUnsplash={"https://unsplash.com/?utm_source=your_app_name&utm_medium=referral"}
          />
        </p>
      </div>
    </div>
  )
}