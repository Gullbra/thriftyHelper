import '../styles/components/hamburger-menu.css'

export const HamburgerMenu = ({callbackFunc}: {callbackFunc: () => void }) => {
  return (
    <>
    <div className="nav-icon1"
      onClick={event => {event.currentTarget.classList.toggle('--hamburger-menu-open'); callbackFunc()}}
    >
      <span></span>
      <span></span>
      <span></span>
    </div>
    </>
  )
}