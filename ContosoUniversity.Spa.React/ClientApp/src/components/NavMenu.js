import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Glyphicon, Nav, Navbar, NavItem, MenuItem, NavDropdown } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';

export class NavMenu extends Component {
  displayName = NavMenu.name

  render() {
    return (
      <Navbar fixedTop inverse>
        <Navbar.Header>
          <Navbar.Brand>
            <Link to={'/'}>Contoso University</Link>
          </Navbar.Brand>
          <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
        <Nav>
        <LinkContainer to={'/'} exact>
              <NavItem>Home</NavItem>
          </LinkContainer>
          <LinkContainer to={'/about'}>
            <NavItem>About</NavItem>
          </LinkContainer>
          <LinkContainer to={'/contact'}>
            <NavItem>Contact</NavItem>
          </LinkContainer>
            <NavDropdown eventKey={3} title="Academic" id="basic-nav-dropdown">
            <LinkContainer to={'/departments'}>
              <MenuItem>
                <Glyphicon glyph='education' /> Departments
              </MenuItem>
            </LinkContainer>
            <LinkContainer to={'/courses'}>
              <MenuItem>
                <Glyphicon glyph='education' /> Courses
              </MenuItem>
            </LinkContainer>
          </NavDropdown>
        </Nav>
        <Nav pullRight>
          <LinkContainer to={'/register'}>
            <NavItem>Register</NavItem>
          </LinkContainer>
          <LinkContainer to={'/login'}>
            <NavItem>Log in</NavItem>    
          </LinkContainer>      
        </Nav>
        </Navbar.Collapse>
      </Navbar>
    );
  }
}
