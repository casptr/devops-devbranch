pipeline {
    agent any

    wsDir=pipelineworkspace
    ws (wsDir) {
        stage 'Workspace'

        echo "Using " + wsDir
        pwd()
        echo 'Cleaning...'
        deleteDir()
    }

    stages {
        stage('Clone the repository') {
            steps {
                echo 'Cloning repository'
                sh 'rm -fr devops-devbranch'
                sh 'git clone https://github.com/casptr/devops-devbranch.git'
            }
            pwd()
        }
        stage('Preparation') {
            steps {
                echo 'Checking database container'
                build job: 'CheckIfDbRunning'
                echo 'Creating application container image'
                build job: 'BuildApp'
            }
        }
        stage('Deployment') {
            steps {
                echo 'Deploying application container'
                build job: 'StartContainer'
            }
        }
    }
}
