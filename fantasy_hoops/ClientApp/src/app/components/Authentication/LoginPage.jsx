import React, {Component} from 'react';
import PropTypes from 'prop-types';
import {Link} from 'react-router-dom';
import {GoogleLogin} from 'react-google-login';
import {Input} from '../Inputs/Input';
import {Alert} from '../Alert';
import {isAuth} from '../../utils/auth';
import {login, googleLogin} from '../../utils/networkFunctions';
import Routes from '../../routes/routes';
import {Helmet} from "react-helmet";
import {Canonicals, Meta} from "../../utils/helpers";
import FullscreenLoader from "../FullscreenLoader";

export default class LoginPage extends Component {
    constructor(props) {
        super(props);
        const {location} = this.props;

        const error = location.state && location.state.error;

        this.state = {
            username: '',
            password: '',
            showAlert: error,
            alertType: error ? 'alert-danger' : '',
            alertText: error ? location.state.error : '',
            showLoader: false
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.responseGoogle = this.responseGoogle.bind(this);
    }

    componentDidMount() {
        // If user has signed in already, he is redirecting to main page
        if (isAuth()) {
            window.location.replace('/');
        }
    }

    responseGoogle = (response) => {
        this.setState({
            showLoader: true
        });
        const {location} = this.props;
        googleLogin(response.tokenId)
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
                    showLoader: false,
                    alertType: 'alert-danger',
                    alertText: err.response.data
                });
            });
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
        this.setState({
            showLoader: true
        });
        e.preventDefault();
        const {username, password} = this.state;
        const {location} = this.props;

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
                    alertText: err.response.data,
                    showLoader: false
                });
            });
    }

    render() {
        const {
            alertType, alertText, showAlert, username, password, showLoader
        } = this.state;
        return (
            <>
                <Helmet>
                    <title>Login | Fantasy Hoops</title>
                    <meta property="title" content="Login | Fantasy Hoops"/>
                    <meta property="og:title" content="Login | Fantasy Hoops"/>
                    <meta property="og:description" content={Meta.DESCRIPTION}/>
                    <meta name="description" content={Meta.DESCRIPTION}/>
                    <meta name="robots" content="noindex,follow"/>
                    <link rel="canonical" href={Canonicals.LOGIN}/>
                </Helmet>
                <div className="container pb-3  vertical-center" style={{maxWidth: '420px'}}>
                    <br/>
                    <h1>Login</h1>
                    <hr/>
                    <Alert type={alertType} text={alertText} show={showAlert}/>
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
                        <button type="submit" id="login" disabled className="btn btn-outline-primary btn-block">Log in
                        </button>
                        <Link to={Routes.REGISTER} className="btn btn-info btn-block">Sign up</Link>
                        <hr/>
                        <GoogleLogin
                            className="btn-block"
                            clientId="742661414003-9j0660djckpdt9rthv18cnb4vo8bq6ch.apps.googleusercontent.com"
                            onSuccess={this.responseGoogle}
                            onFailure={this.responseGoogle}
                            cookiePolicy="single_host_origin"
                        />
                    </form>
                </div>
                {showLoader && <FullscreenLoader />}
            </>
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
