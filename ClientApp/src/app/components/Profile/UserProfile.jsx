import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { parse } from '../../utils/auth';
import { Avatar } from './Avatar';
import { EditProfile } from './EditProfile';
import { InfoPanel } from './InfoPanel';
import { Friends } from './Friends/Friends';
import { Error } from '../Error';
import { getUserData, getUserDataByName } from '../../utils/networkFunctions';

export class UserProfile extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);

    const { match } = this.props;
    this.state = {
      edit: match.params.edit || '',
      user: '',
      readOnly: true,
      error: false,
      errorStatus: '200',
      errorMessage: '',
      loader: true
    };
  }

  async componentDidMount() {
    this._isMounted = true;

    const { match } = this.props;
    const loggedInAsSameUser = (
      match.params.name != null && parse()
        .username.toLowerCase() === match.params.name.toLowerCase()
    );
    if (match.params.name == null || loggedInAsSameUser) {
      const user = parse();

      await getUserData(user.id, { count: 5 })
        .then((res) => {
          this.setState({
            user: res.data,
            loader: false,
            readOnly: false
          });
        });
      this.editProfile();
    } else {
      const userName = match.params.name;
      if (this._isMounted) {
        this.setState({
          readOnly: true
        });
      }
      await getUserDataByName(userName, { count: 5 })
        .then((res) => {
          if (this._isMounted) {
            this.setState({
              user: res.data,
              loader: false,
              readOnly: true
            });
          }
        })
        .catch((err) => {
          if (this._isMounted) {
            this.setState({
              error: true,
              errorStatus: err.response.status,
              errorMessage: err.response.data,
              loader: false
            });
          }
        });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  editProfile() {
    const { edit } = this.state;
    if (edit === 'edit') {
      const editTab = document.getElementById('edit');
      const profileTab = document.getElementById('profile');
      const friendsTab = document.getElementById('friends');
      const editLink = document.getElementById('navLinkEdit');
      const profileLink = document.getElementById('navLinkProfile');
      const friendsLink = document.getElementById('navLinkFriends');
      profileTab.className = 'tab-pane';
      friendsTab.className = 'tab-pane';
      editTab.className = 'tab-pane active show';
      profileLink.className = 'nav-link btn-no-outline';
      friendsLink.className = 'nav-link btn-no-outline';
      editLink.className = 'nav-link active show btn-no-outline';
    }
  }

  render() {
    const {
      error, errorStatus, errorMessage, readOnly, user, loader
    } = this.state;
    if (error) {
      return (
        <Error status={errorStatus} message={errorMessage} />
      );
    }

    const content = () => (
      <div className="col-lg-8 order-lg-2 mt-5 p-2">
        <ul className="nav nav-pills">
          <li className="nav-item">
            <Link to="" data-target="#profile" data-toggle="tab" id="navLinkProfile" className="nav-link active tab-no-outline">Profile</Link>
          </li>
          <li className="nav-item">
            <Link to="" data-target="#friends" data-toggle="tab" id="navLinkFriends" className="nav-link tab-no-outline">Friends</Link>
          </li>
          {!readOnly
            && (
              <li className="nav-item">
                <Link to="" data-target="#edit" data-toggle="tab" id="navLinkEdit" className="nav-link tab-no-outline">Edit</Link>
              </li>
            )
          }
        </ul>
        <div className="tab-content py-4">
          <InfoPanel user={user} readOnly={readOnly} />
          <Friends user={user} />
          <EditProfile user={user} />
        </div>
      </div>
    );
    if (loader) {
      return (
        <div className="mx-auto mt-5">
          <div className="Loader" />
        </div>
      );
    }

    return (
      <div className="container bg-light">
        <div className="row mx-auto">
          <div className="col-lg-4 order-lg-1 p-0 pt-3">
            {user ? <Avatar user={user} readOnly={readOnly} /> : ''}
          </div>
          {content()}
        </div>
      </div>
    );
  }
}

UserProfile.propTypes = {
  match: PropTypes.shape().isRequired
};

export default UserProfile;
