import '../styles/components/hamburger-menu.css'

export const HamburgerMenu = ({callbackFunc}: {callbackFunc: () => void}) => {
  const clickHandler = (event: React.MouseEvent<HTMLDivElement, MouseEvent> | React.KeyboardEvent<HTMLDivElement>) => {
    event.currentTarget.classList.toggle('--hamburger-menu-open')
    callbackFunc()
  }

  return (
    <>
      <div className={`nav-icon1 --hamburger-menu-open`}
        tabIndex={0}
        onKeyUp={event => { if(event.code.toLocaleLowerCase() === "enter") {clickHandler(event)} }}
        onClick= {clickHandler}
      >
        <span/><span/><span/>
      </div>
    </>
  )
}

// from https://loading.io/css/