import React from "react";
import ReactNotification from "react-notifications-component";
import "react-notifications-component/dist/theme.css";

export class AlertNotification extends React.Component {
  constructor(props) {
    super(props);
    this.addNotification = this.addNotification.bind(this);
    this.notificationDOMRef = React.createRef();
  }

  addNotification() {
    const title = this.props.type === 'success'
      ? 'Success!'
      : 'Error!';
    this.notificationDOMRef.current.addNotification({
      title: title,
      message: this.props.text,
      type: this.props.type,
      insert: "top",
      container: "top-right",
      animationIn: ["animated", "flipInX"],
      animationOut: ["animated", "flipOutX"],
      dismiss: { duration: 5000 },
      dismissable: { click: true, touch: true }
    });
  }

  render() {
    return <ReactNotification ref={this.notificationDOMRef} />;
  }
}