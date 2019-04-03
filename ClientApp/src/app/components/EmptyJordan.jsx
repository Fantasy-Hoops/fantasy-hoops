import React from 'react';
import PropTypes from 'prop-types';

const EmptyJordan = (props) => {
  const { message, className } = props;
  return (
    <div className={`text-center ${className}`}>
      <img className="text-center" src={require('../../content/images/jordan-crying.png')} style={{ height: '12.5rem' }} alt="Sad Jordan" />
      <h3>{message}</h3>
    </div>
  );
};

EmptyJordan.propTypes = {
  message: PropTypes.string.isRequired,
  className: PropTypes.string
};

EmptyJordan.defaultProps = {
  className: ''
};

export default EmptyJordan;
