import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Employee } from '../../models/employee';

interface ChatMessage {
  sender: 'user' | 'agent';
  text: string;
}

@Component({
  selector: 'app-employee-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-chat.component.html',
  styleUrl: './employee-chat.component.css'
})
export class EmployeeChatComponent implements OnInit {
  employees: Employee[] = [];
  selectedEmployeeId = 0;
  userMessage = '';
  messages: ChatMessage[] = [];
  sending = false;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getEmployees().subscribe({
      next: (data) => this.employees = data
    });
  }

  send(): void {
    if (!this.userMessage.trim() || this.selectedEmployeeId === 0) return;

    this.messages.push({ sender: 'user', text: this.userMessage });
    const msg = this.userMessage;
    this.userMessage = '';
    this.sending = true;

    this.apiService.sendChatMessage(this.selectedEmployeeId, msg).subscribe({
      next: (res) => {
        this.messages.push({ sender: 'agent', text: res.reply });
        this.sending = false;
      },
      error: () => {
        this.messages.push({ sender: 'agent', text: 'Sorry, something went wrong. Please try again.' });
        this.sending = false;
      }
    });
  }
}
