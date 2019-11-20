import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Input } from '../Inputs/Input';
import { Alert } from '../Alert';
import { isAuth } from '../../utils/auth';
import { register } from '../../utils/networkFunctions';
import Routes from '../../routes/routes';

export class Registration extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      email: '',
      password: '',
      confirmPassword: '',
      showAlert: false,
      alertType: '',
      alertText: ''
    }
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  handleChange(e) {
    this.setState({
      [e.target.id]: e.target.value
    });

    const btn = document.getElementById('submit');
    if (document.querySelectorAll('.is-invalid').length !== 0) {
      btn.className = 'btn btn-outline-primary btn-block';
      btn.disabled = true;
      return;
    }
    else {
      const forms = document.querySelectorAll('.form-control');
      for (let i = 0; i < forms.length; i++) {
        if (!forms[i].required)
          continue;
        if (forms[i].value.length === 0) {
          btn.className = 'btn btn-outline-primary btn-block';
          btn.disabled = true;
          return;
        }
      }
    }
    btn.className = 'btn btn-primary btn-block';
    btn.disabled = false;
  }

  handleSubmit(e) {
    document.getElementById('submit').disabled = true;
    e.preventDefault();
    const data = {
      UserName: this.state.username,
      Email: this.state.email,
      Password: this.state.password
    };

    register(data)
      .then(res => {
        this.setState({
          showAlert: true,
          alertType: 'alert-info',
          alertText: res.data
        });
        document.getElementById('submit').remove();
        document.getElementById('form').remove();
      })
      .catch(err => {
        document.getElementById('submit').disabled = false;
        this.setState({
          showAlert: true,
          alertType: 'alert-danger',
          alertText: err.response.data
        });
      });
  }

  componentDidMount() {
    // If user has signed in already, he is redirecting to main page
    if (isAuth()) {
      window.location.replace("/");
    }
  }

  render() {
    return (
      <div className="container pb-3 bg-light vertical-center" style={{ maxWidth: '420px' }}>
        <br />
        <h2>Registration</h2>
        <hr />
        <Alert type={this.state.alertType} text={this.state.alertText} show={this.state.showAlert} />
        <form onSubmit={this.handleSubmit} id="form">
          <div className="form-group">
            <label>Username</label>
            <Input
              type="text"
              id="username"
              value={this.state.username}
              onChange={this.handleChange}
              regex={/^.{4,11}$/}
              error="Username must be between 4 and 11 symbols long"
            />
          </div>
          <div className="form-group">
            <label>Email</label>
            <Input
              type="email"
              id="email"
              value={this.state.email}
              onChange={this.handleChange}
              regex={/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/}
              error="Invalid email"
            />
          </div>
          <div className="form-group">
            <label>Password</label>
            <Input
              type="password"
              id="password"
              value={this.state.password}
              onChange={this.handleChange}
              regex={/^.{8,20}$/}
              error="Password must contain 8-20 characters."
              children="confirmPassword"
            />
          </div>
          <div className="form-group">
            <label>Confirm password</label>
            <Input
              type="password"
              id="confirmPassword"
              value={this.state.confirmPassword}
              onChange={this.handleChange}
              match="password"
              error="Passwords must match"
            />
          </div>
          <button id="submit" disabled className="btn btn-outline-primary btn-block">Submit</button>
        </form>
        <div className="mt-1">
          <small style={{ color: 'hsl(0, 0%, 45%)' }}>
            Already on Fantasy Hoops?
            <Link to={Routes.LOGIN}> Sign In</Link>
          </small>
        </div>
      </div>
    );
  }


}

export default Registration;
