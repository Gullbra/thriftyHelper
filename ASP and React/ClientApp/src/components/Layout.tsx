import React from 'react';
// import { Container } from 'reactstrap';
import {NavMenu} from './NavMenu';

const Layout = (
  { children }: {children: React.ReactNode}
) => {
  return (
    <div>
      <NavMenu />
      {children ? children : "Nothing here"}
      {/* <Container tag="main">
        {this.props.children}
      </Container> */}
    </div>
  );
}

export default Layout
