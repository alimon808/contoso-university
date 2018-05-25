import React from 'react';
import { Col, Form, FormGroup, ControlLabel, FormControl, Button, HelpBlock } from 'react-bootstrap';
import './form.css';

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
                fieldValidationErrors.email = emailValid ? '' : 'Email is not valid';
                break;
            case 'password':
                passwordValid = value.length >= 6;
                fieldValidationErrors.password = passwordValid ? '' : 'Password is too short';
                break;
            case 'confirmPassword':
                confirmPasswordValid = this.state.password === value;
                fieldValidationErrors.confirmPassword = confirmPasswordValid ? '' : 'ConfirmPassword does not match password';
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
        const formErrors = this.state.formErrors;
        return (
            <Form horizontal>
                <h4>Create a new account</h4>
                <hr />
                <FormGroup validationState={this.validationState(formErrors.email)}>
                    <Col componentClass={ControlLabel} sm={2}>Email</Col>
                    <Col sm={10}>
                        <FormControl name="email" type="email" value={this.state.email} onChange={this.handleChange} />
                        <FormControl.Feedback />
                        <HelpBlock>{formErrors.email}</HelpBlock>
                    </Col>
                </FormGroup>
                <FormGroup validationState={this.validationState(formErrors.password)}>
                    <Col componentClass={ControlLabel} sm={2}>Password</Col>
                    <Col sm={10}><FormControl type="password" name="password" value={this.state.password} onChange={this.handleChange} />
                        <FormControl.Feedback />
                        <HelpBlock>{formErrors.password}</HelpBlock>
                    </Col>
                </FormGroup>
                <FormGroup validationState={this.validationState(formErrors.confirmPassword)}>
                    <Col componentClass={ControlLabel} sm={2}>Confirm Password</Col>
                    <Col sm={10}><FormControl type="password" name="confirmPassword" value={this.state.confirmPassword} onChange={this.handleChange} />
                        <FormControl.Feedback />
                        <HelpBlock>{formErrors.confirmPassword}</HelpBlock>
                    </Col>
                </FormGroup>
                <FormGroup>
                    <Col smOffset={2} sm={10}><Button type="submit" disabled={!this.state.formValid} onClick={this.handleClick}>Register</Button></Col>
                </FormGroup>
            </Form >
        );
    };

}