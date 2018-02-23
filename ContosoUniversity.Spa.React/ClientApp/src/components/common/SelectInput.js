import React from 'react';
import PropTypes from 'prop-types';

const SelectInput = ({label, name, value, onChange, defaultOption, options, error}) => {
    return (
        <div className="form-group">
            <label htmlFor={name}>{label}</label>
            <div className="field">
                <select
                    name={name}
                    value={value}
                    onChange={onChange}
                    className="form-control">
                    <option value="">{defaultOption}</option>
                    {options.map(
                        (option) => {
                            return <option key={option.value} value={option.value}>{option.text}</option>;
                        }
                    )}
                </select>
                {error && <div className="alert alert-danger">{error}</div>}
            </div>
        </div>
    );
}

SelectInput.propTypes = {
    label: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    value: PropTypes.number,
    onChange: PropTypes.func.isRequired,
    defaultOption: PropTypes.string,
    options: PropTypes.arrayOf(PropTypes.object),
    error: PropTypes.string,
}

export default SelectInput;