import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import { NavMenu } from './NavMenu';
import './Layout.css';
import Footer from './Footer';


export class Layout extends Component {
  displayName = Layout.name
  render() {
    return (
      <div>
        <NavMenu />
        <Grid className="body-content">
          <Row>
            <Col md={10}>
              {this.props.children}
            </Col>
          </Row>
          <Row>
            <Col md={10}>
              <Footer />
            </Col>
          </Row>
        </Grid>
      </div>
    );
  }
}
