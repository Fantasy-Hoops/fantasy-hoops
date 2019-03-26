import React, { Component } from 'react';
import Avatar from 'react-avatar-edit';
import { parse } from '../../utils/auth';
import { Alert } from '../Alert';
import { uploadAvatar, clearAvatar } from '../../utils/networkFunctions';

export class ChangeAvatar extends Component {
  constructor(props) {
    super(props);

    this.onCrop = this.onCrop.bind(this);
    this.closeImage = this.closeImage.bind(this);
    this.loadImage = this.loadImage.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleClear = this.handleClear.bind(this);

    this.state = {
      preview: null,
      showAlert: false,
      alertType: '',
      alertText: ''
    }
  }

  componentDidMount() {
    const saveBtn = document.getElementById('save');
    saveBtn.disabled = false;
  }

  closeImage() {
    this.setState({
      preview: null,
      showAlert: false,
      alertType: '',
      alertText: ''
    });
  }

  clear() {
    this.setState({
      preview: null,
      src: null
    });
    if (this.state.showAlert)
      window.location.reload();
  }

  loadImage() {
    const saveBtn = document.getElementById('save');
    saveBtn.disabled = false;
  }

  onCrop(preview) {
    this.setState({ preview })
  }

  handleSubmit() {
    const user = parse();
    const data = {
      id: user.id,
      avatar: this.state.preview
    }

    uploadAvatar(data)
      .then(res => {
        this.setState({
          showAlert: true,
          alertType: 'alert-success',
          alertText: res.data
        });
      })
      .catch(err => {
        this.setState({
          showAlert: true,
          alertType: 'alert-danger',
          alertText: err.response.data
        });
      });
  }

  handleClear() {
    const saveBtn = document.getElementById('save');
    saveBtn.disabled = true;
    const user = parse();
    const data = {
      id: user.id
    }

    clearAvatar(data)
      .then(res => {
        this.setState({
          showAlert: true,
          alertType: 'alert-success',
          alertText: res.data
        });
      })
      .catch(err => {
        this.setState({
          showAlert: true,
          alertType: 'alert-danger',
          alertText: err.response.data
        });
      });
  }

  render() {
    const clearAvatar = <div>
      <div className="text-center mt-2 mb-2">or</div>
      <div className="text-center">
        <button onClick={this.handleClear} type="button" className="btn btn-outline-danger">Clear avatar</button>
      </div>
    </div>;
    return (
      <div className="modal fade" id="changeImage" tabIndex="-1" role="dialog" aria-labelledby="changeImage" aria-hidden="true">
        <div className="modal-dialog" role="document">
          <div className="modal-content">
            <div className="modal-header">
              <h2 className="modal-title" id="changeImage">Crop your image</h2>
              <a type="button btn-no-outline" className="close" data-dismiss="modal" aria-label="Close">
                <span aria-hidden="true">&times;</span>
              </a>
            </div>
            <div className="modal-body mx-auto">
              <Alert type={this.state.alertType} text={this.state.alertText} show={this.state.showAlert} />
              <Avatar
                width={350}
                height={250}
                onCrop={this.onCrop}
                onClose={this.closeImage}
                onFileLoad={this.loadImage}
              />
              {!this.state.preview ? clearAvatar : ''}
            </div>
            <div className="modal-footer mx-auto">
              <button disabled={false} id="save" onClick={this.handleSubmit} type="button" className="btn btn-info">Save changes</button>
              <button type="button" className="btn btn-primary" data-dismiss="modal">Close</button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}