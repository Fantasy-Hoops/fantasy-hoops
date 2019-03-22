import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { Input } from '../Inputs/Input';
import { Alert } from '../Alert';
import { isAuth } from '../../utils/auth';
import { login } from '../../utils/networkFunctions';

export default class LoginPage extends Component {
  constructor(props) {
    super(props);
    const { location } = this.props;

    const error = location.state && location.state.error;

    this.state = {
      username: '',
      password: '',
      showAlert: error,
      alertType: error ? 'alert-danger' : '',
      alertText: error ? location.state.error : ''
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {
    // If user has signed in already, he is redirecting to main page
    if (isAuth()) {
      window.location.replace('/');
    }
  }

  handleChange(e) {
    this.setState({
      [e.target.id]: e.target.value
    });

    const btn = document.getElementById('login');
    if (document.querySelectorAll('.is-invalid').length !== 0) {
      btn.className = 'btn btn-outline-primary btn-block';
      btn.disabled = true;
      return;
    }
    const forms = document.querySelectorAll('.form-control');
    for (let i = 0; i < forms.length; i++) {
      if (!forms[i].required) {
        continue;
      }
      if (forms[i].value.length === 0) {
        btn.className = 'btn btn-outline-primary btn-block';
        btn.disabled = true;
        return;
      }
    }
    btn.className = 'btn btn-primary btn-block';
    btn.disabled = false;
  }

  handleSubmit(e) {
    e.preventDefault();
    const { username, password } = this.state;
    const { location } = this.props;

    const data = {
      UserName: username,
      Password: password
    };

    login(data)
      .then((res) => {
        localStorage.setItem('accessToken', res.data);

        // If user was redirected to login because of authentication errors,
        // he is now being redirected back
        if (location.state && location.state.fallback) {
          window.location.replace('/lineup');
          return;
        }
        window.location.replace('/');
      })
      .catch((err) => {
        this.setState({
          showAlert: true,
          alertType: 'alert-danger',
          alertText: err.response.data
        });
      });
  }

  render() {
    const {
      alertType, alertText, showAlert, username, password
    } = this.state;
    return (
      <div className="container pb-3 bg-light vertical-center" style={{ maxWidth: '420px' }}>
        <br />
        <h2>Login</h2>
        <hr />
        <Alert type={alertType} text={alertText} show={showAlert} />
        <form onSubmit={this.handleSubmit} id="form">
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <Input
              type="text"
              id="username"
              placeholder="Username"
              value={username}
              onChange={this.handleChange}
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <Input
              type="password"
              id="password"
              placeholder="Password"
              value={password}
              onChange={this.handleChange}
            />
          </div>
          <button type="submit" id="login" disabled className="btn btn-outline-primary btn-block">Log in</button>
          <Link to="/register" className="btn btn-success btn-block">Sign up</Link>
        </form>
      </div>
    );
  }
}

LoginPage.propTypes = {
  location: PropTypes.shape({
    pathname: PropTypes.string.isRequired
  })
};

LoginPage.defaultProps = {
  location: {
    pathname: ''
  }
};
