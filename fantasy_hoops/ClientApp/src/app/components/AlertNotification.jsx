import React from 'react';
import PropTypes from 'prop-types';
import ReactNotification from 'react-notifications-component';
import 'react-notifications-component/dist/theme.css';
import _ from 'lodash';

export class AlertNotification extends React.Component {
  constructor(props) {
    super(props);
    this.addNotification = this.addNotification.bind(this);
    this.notificationDOMRef = React.createRef();
  }

  addNotification() {
    const { type, text } = this.props;
    const title = type === 'success'
      ? 'Success!'
      : 'Error!';
    this.notificationDOMRef.current.addNotification({
      title,
      message: !_.isEmpty(text) ? text : 'Error',
      type,
      insert: 'top',
      container: 'top-right',
      animationIn: ['animated', 'flipInX'],
      animationOut: ['animated', 'flipOutX'],
      dismiss: { duration: 5000 },
      dismissable: { click: true, touch: true }
    });
  }

  render() {
    return <ReactNotification ref={this.notificationDOMRef} />;
  }
}

AlertNotification.propTypes = {
  type: PropTypes.string.isRequired,
  text: PropTypes.string.isRequired
};

export default AlertNotification;
