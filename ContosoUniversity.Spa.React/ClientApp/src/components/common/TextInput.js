import React from 'react';
import PropTypes from 'prop-types';

const TextInput = ({name, label, placeholder, value, onChange, error}) => {
    let wrapperClass = 'form-group';
    if(error && error.length > 0){
        wrapperClass += " " + 'has-error';
    }
    
    return (
        <div className="wrapperClass">
            <label htmlFor={name}>{label}</label>
            <div className="field">
                <input 
                    type="text"
                    name={name}
                    className="form-control"
                    placeHolder={placeHolder}
                    value={value}
                    onChange={onChange}
                />
                {error && <div className="alert alert-danger">{error}</div>}
            </div>
        </div>
    );
}

TextInput.propTypes = {
    name: PropTypes.string.isRequired,
    label: PropTypes.string.isRequired,
    placeHolder: PropTypes.string,
    value: PropTypes.string,
    onChange: PropTypes.func.isRequired,
    error: PropTypes.string
};