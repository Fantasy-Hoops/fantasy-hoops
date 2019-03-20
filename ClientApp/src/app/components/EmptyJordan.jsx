import React, { Component } from 'react';
import PropTypes from 'prop-types';

export class EmptyJordan extends Component {
  constructor(props) {
    super(props);
    this.state = {};
  }

  render() {
    const { message } = this.props;
    return (
      <div className="text-center">
        <img className="text-center" src={require('../../content/images/jordan-crying.png')} style={{ height: '12.5rem' }} alt="Sad Jordan" />
        <h3>{message}</h3>
      </div>
    );
  }
}

EmptyJordan.propTypes = {
  message: PropTypes.string.isRequired
};

export default EmptyJordan;
