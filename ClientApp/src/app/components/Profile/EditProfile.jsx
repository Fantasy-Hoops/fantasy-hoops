import React, { Component } from 'react';
import Textarea from 'react-autosize-textarea';
import { Input } from '../Inputs/Input';
import { Select } from '../Inputs/Select';
import { parse } from '../../utils/auth';
import { Alert } from '../Alert';
import { editProfile, getTeams } from '../../utils/networkFunctions';

export class EditProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isChangingPassword: false,
      username: '',
      email: '',
      about: '',
      password: '',
      newPassword: '',
      confirmNewPassword: '',
      team: '',
      teams: '',
      showAlert: false,
      alertType: '',
      alertText: ''
    };

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.initChangePassword = this.initChangePassword.bind(this);
  }

  async componentDidMount() {
    await getTeams()
      .then((res) => {
        this.setState({
          teams: res.data,
          username: this.props.user.userName,
          email: this.props.user.email,
          about: this.props.user.description || '',
          team: this.props.user.favoriteTeamId
        });
      });
  }

  componentDidUpdate(nextProps, nextState) {
    if (this.props !== nextProps) {
      this.setState({
        username: this.props.user.userName,
        email: this.props.user.email,
        about: this.props.user.description || '',
        team: this.props.user.favoriteTeamId
      })
    }
    const btn = document.getElementById('submit');
    if (document.querySelectorAll('.is-invalid').length !== 0) {
      btn.className = 'btn btn-success';
      btn.disabled = true;
      return;
    }

    const forms = document.querySelectorAll('.form-control');
    for (let i = 0; i < forms.length; i++) {
      if (!forms[i].required)
        continue;
      if (forms[i].value.length === 0) {
        btn.className = 'btn btn-success';
        btn.disabled = true;
        return;
      }
    }

    btn.className = 'btn btn-success';
    btn.disabled = false;
  }

  handleChange(e) {
    this.setState({
      [e.target.id]: e.target.value
    });
  }

  handleSubmit(e) {
    e.preventDefault();
    const user = parse();
    const data = {
      Id: user.id,
      UserName: this.state.username,
      Email: this.state.email,
      Description: this.state.about,
      FavoriteTeamId: this.state.team,
      CurrentPassword: this.state.password,
      NewPassword: this.state.newPassword,
    };

    editProfile(data)
      .then((res) => {
        localStorage.setItem('accessToken', res.data);
        window.location.reload();
      })
      .catch((err) => {
        this.setState({
          showAlert: true,
          alertType: 'alert-danger',
          alertText: err.response.data
        });
      });
  }

  initChangePassword() {
    this.setState({
      isChangingPassword: true
    });
  }

  render() {
    const { teams } = this.state;
    const changingPassword = !(this.state.password.length > 0
      || this.state.newPassword.length > 0
      || this.state.confirmNewPassword.length > 0);
    return (
      <div className="tab-pane" id="edit">
        <Alert type={this.state.alertType} text={this.state.alertText} show={this.state.showAlert} />
        <form onSubmit={this.handleSubmit}>
          <label className="col-lg-3 col-form-label form-control-label" />
          <div className="col-lg-9">
            <label className="form-group row">PERSONAL INFO</label>
          </div>
          <hr className="col-xs-12" />
          <div className="form-group row">
            <label className="col-lg-3 col-form-label form-control-label">Username</label>
            <div className="col-lg-9">
              <Input
                type="text hidden"
                id="username"
                value={this.state.username}
                onChange={this.handleChange}
                regex={/^.{4,11}$|^$/}
                error="Username must be between 4 and 11 symbols long"
              />
            </div>
          </div>
          <div className="form-group row">
            <label className="col-lg-3 col-form-label form-control-label">Email</label>
            <div className="col-lg-9">
              <Input
                type="email hidden"
                id="email"
                value={this.state.email}
                onChange={this.handleChange}
                regex={/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/}
                error="Invalid email"
              />
            </div>
          </div>
          <div className="form-group row">
            <label className="col-lg-3 col-form-label form-control-label">Favorite team</label>
            <div className="col-lg-9">
              <Select
                options={teams}
                id="team"
                value={this.state.team}
                onChange={this.handleChange}
              />
            </div>
          </div>
          <div className="form-group row">
            <label className="col-lg-3 col-form-label form-control-label">About me</label>
            <div className="col-lg-9">
              <div className="form-group">
                <Textarea
                  id="about"
                  className="form-control"
                  value={this.state.about}
                  onChange={this.handleChange}
                />
              </div>
            </div>
          </div>
          <label className="col-lg-3 col-form-label form-control-label" />
          {this.state.isChangingPassword
            && <div className="col-lg-9">
              <label className="form-group row">CHANGE PASSWORD</label>
            </div>
          }
          {this.state.isChangingPassword
            && <div>
              <hr className="col-xs-12" />
              <div className="form-group row">
                <label className="col-lg-3 col-form-label form-control-label">Password</label>
                <div className="col-lg-9">
                  <Input
                    type="password"
                    id="password"
                    value={this.state.password}
                    onChange={this.handleChange}
                    notRequired={changingPassword}
                  />
                </div>
              </div>
              <div className="form-group row">
                <label className="col-lg-3 col-form-label form-control-label">New password</label>
                <div className="col-lg-9">
                  <Input
                    type="password"
                    id="newPassword"
                    value={this.state.newPassword}
                    onChange={this.handleChange}
                    regex={/^.{8,20}$/}
                    error="Password must contain 8-20 characters."
                    notRequired={changingPassword}
                    children="confirmNewPassword"
                  />
                </div>
              </div>
              <div className="form-group row">
                <label className="col-lg-3 col-form-label form-control-label">Confirm new password</label>
                <div className="col-lg-9">
                  <Input
                    type="password"
                    id="confirmNewPassword"
                    value={this.state.confirmNewPassword}
                    onChange={this.handleChange}
                    match="newPassword"
                    error="Passwords must match"
                    notRequired={changingPassword}
                  />
                </div>
              </div>
            </div>}
          <div className="form-group row">
            <label className="col-lg-3 col-form-label form-control-label" />
            <div className="col-lg-9">
              <button id="submit" disabled className="btn btn-secondary">Save changes</button>
              {!this.state.isChangingPassword && <button type="button" onClick={this.initChangePassword} className="btn btn-outline-primary ml-2">Change Password</button>}
            </div>
          </div>
        </form>
      </div>
    );
  }
}
