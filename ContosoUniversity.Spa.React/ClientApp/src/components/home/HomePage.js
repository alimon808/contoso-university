import React from 'react';
import { Jumbotron, Grid, Row, Col } from 'react-bootstrap';

export default function () {
    return (
        <div class="container">
            <Jumbotron>
                <h1>Contoso University</h1>
                <p>Welcome to your new single-page application, built with:</p>
                <ul>
                    <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
                    <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
                    <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
                    <li><a href='https://github.com/alimon808/contoso-university/tree/master/ContosoUniversity.Spa.React'>Source Code</a> on Github</li>
                </ul>
            </Jumbotron>
            <Grid>
                <Row>
                    <Col md={4}>
                        <p>
                            Contoso University is a place for learning AspNetCore and related technologies.  This demo application is an amalgamation of smaller demo applications found in tutorials at <a href="https://docs.microsoft.com/en-us/aspnet/core/">AspNetCore docs</a>.  The tutorials are great at demonstrating isolated concepts, but issues surfaces when applying these concepts/techniques in a larger context.  The purpose of this demo application is to apply concepts/techniques learned from those tutorial into a single domain (i.e. university).
                        </p>
                    </Col>
                    <Col md={4}>
                        <h2>Build it from scratch</h2>
                        <p>
                            You can build the initial application by following the steps in a series of tutorials.
                        </p>
                        <p>
                            <a class="btn btn-default" href="https://docs.microsoft.com/en-us/aspnet/core/spa/react?view=aspnetcore-2.1&tabs=visual-studio">Seee the tutorial &raquo;</a>
                        </p>
                    </Col>
                    <Col md={4}>
                        <h2>Download it</h2>
                        <p>You can download the completed project from Github.</p>
                        <p>
                            <a class="btn btn-default" href="https://github.com/alimon808/contoso-university/tree/master/ContosoUniversity.Spa.React">See project source code &raquo;</a>
                        </p>
                    </Col>
                </Row>
            </Grid>
        </div>
    );
}