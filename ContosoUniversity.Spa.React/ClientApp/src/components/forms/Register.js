// todo
// add password validation
// add confirmPassword validation

import React from 'react';
import { Col, Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import './form.css';

function FormErrors(formErrors) {
    return (
        <div className='formErrors'>
            {Object.keys(formErrors).map((fieldName, i) => {
                if (formErrors[fieldName].length > 0) {
                    return (
                        <p key={i}>{fieldName} {formErrors[fieldName]}</p>
                    )
                } else {
                    return '';
                }
            })}
        </div>
    );
}

export default class Register extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            email: '',
            password: '',
            confirmPassword: '',
            formErrors: { email: '', password: '', confirmPassword: '' },
            emailValid: false,
            passwordValid: false,
            confirmPasswordValid: false,
            formValid: false
        };
        this.handleClick = this.handleClick.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }

    handleClick(event) {
        event.preventDefault();
    }

    handleChange(event) {
        const name = event.target.name;
        const value = event.target.value;
        this.setState({ [name]: value }, () => { this.validateField(name, value) });
    }

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;
        let emailValid = this.state.emailValid;
        let passwordValid = this.state.passwordValid;
        let confirmPasswordValid = this.state.confirmPasswordValid;

        switch (fieldName) {
            case 'email':
                emailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i);
                fieldValidationErrors.email = emailValid ? '' : ' is invalid';
                break;
            case 'password':
                passwordValid = value.length >= 6;
                fieldValidationErrors.password = passwordValid ? '' : ' is too short';
                break;
            case 'confirmPassword':
                confirmPasswordValid = this.state.password === value;
                fieldValidationErrors.confirmPassword = confirmPasswordValid ? '' : ' does not match password';
            default:
                break;
        }

        this.setState({
            formErrors: fieldValidationErrors,
            emailValid: emailValid,
            passwordValid: passwordValid,
            confirmPasswordValid: confirmPasswordValid
        }, this.validateForm);
    }

    validateForm() {
        const formValid = this.state.emailValid && this.state.passwordValid && this.state.confirmPasswordValid;
        this.setState({ formValid: formValid });
    }

    validationState(error) {
        return (error.length === 0 ? null : 'error');
    }

    render() {
        return (
            <Form horizontal>
                <h4>Create a new account</h4>
                <hr />
                <FormGroup validationState={this.validationState(this.state.formErrors.email)}>
                    <Col componentClass={ControlLabel} sm={2}>Email</Col>
                    <Col sm={10}>
                        <FormControl name="email" type="email" value={this.state.email} onChange={this.handleChange} />
                        <FormControl.Feedback />
                    </Col>
                </FormGroup>
                <FormGroup validationState={this.validationState(this.state.formErrors.password)}>
                    <Col componentClass={ControlLabel} sm={2}>Password</Col>
                    <Col sm={10}><FormControl type="password" name="password" value={this.state.password} onChange={this.handleChange} />
                    <FormControl.Feedback />
                    </Col>
                </FormGroup>
                <FormGroup validationState={this.validationState(this.state.formErrors.confirmPassword)}>
                    <Col componentClass={ControlLabel} sm={2}>Confirm Password</Col>
                    <Col sm={10}><FormControl type="password" name="confirmPassword" value={this.state.confirmPassword} onChange={this.handleChange} />
                        <FormControl.Feedback />
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col componentClass={ControlLabel} sm={2}></Col>
                    <Col sm={10}><Button type="submit" disabled={!this.state.formValid} onClick={this.handleClick}>Register</Button></Col>
                </FormGroup>
                <div className="panel panel-default">
                    <FormErrors formErrors={this.state.formErrors} />
                </div>
            </Form >
        );
    };

}